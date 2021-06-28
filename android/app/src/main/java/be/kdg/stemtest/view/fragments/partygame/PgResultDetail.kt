package be.kdg.stemtest.view.fragments.partygame

import androidx.lifecycle.ViewModelProviders
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import be.kdg.stemtest.viewmodel.PgResultDetailViewModel
import be.kdg.stemtest.R


class PgResultDetail : Fragment() {


    private lateinit var viewModel: PgResultDetailViewModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.pg_result_detail_fragment, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)
        viewModel = ViewModelProviders.of(this).get(PgResultDetailViewModel::class.java)
    }



}
