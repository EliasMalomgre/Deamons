package be.kdg.stemtest.view.fragments.partygame

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.ViewModelProviders
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import be.kdg.stemtest.R
import be.kdg.stemtest.viewmodel.PartyViewModel
import dagger.android.AndroidInjector
import dagger.android.DispatchingAndroidInjector
import dagger.android.HasAndroidInjector
import dagger.android.support.AndroidSupportInjection
import javax.inject.Inject


class SelectParty : Fragment(),HasAndroidInjector {

    private lateinit var viewManager: LinearLayoutManager
    private lateinit var viewAdapter : PartyAdapter
    private lateinit var recyclerView: RecyclerView

    @Inject  lateinit var androidInjector: DispatchingAndroidInjector<Any>
    @Inject  lateinit var viewModelFactory: ViewModelProvider.Factory

    companion object {
        fun newInstance() = SelectParty()
    }

    private lateinit var viewModel: PartyViewModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?): View? {
        val view = inflater.inflate(
            R.layout.selecteer_partij_fragment, container
        , false)
        return view

    }
    override fun onAttach(context: Context) {
        AndroidSupportInjection.inject(this)
        super.onAttach(context)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        viewModel = ViewModelProviders.of(this, viewModelFactory)[PartyViewModel::class.java]
        initialiseViews(view)
        setRecyclerData()
//        initialiseViews(view)
    }

    private fun setRecyclerData() {
        viewModel.getAllParties()
            .observe(viewLifecycleOwner, Observer {
                    i -> viewAdapter.setData(i)
                recyclerView.adapter=viewAdapter
            })
    }


    private fun initialiseViews(view: View) {
        viewManager = LinearLayoutManager(context)
        viewAdapter =
            PartyAdapter(context, viewModel)
        recyclerView = view.findViewById(R.id.partijRv)
        recyclerView.apply {
            setHasFixedSize(true)
            adapter = viewAdapter
            layoutManager = viewManager
        }
    }

    override fun androidInjector(): AndroidInjector<Any> {
        return androidInjector
    }




}
