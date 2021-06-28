package be.kdg.stemtest.viewmodel

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import be.kdg.stemtest.model.entity.Party
import be.kdg.stemtest.model.repositories.PartyRepository
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class PartyViewModel @Inject constructor(private val partyRepository: PartyRepository
) : ViewModel() {

    fun getAllParties(): LiveData<List<Party>> {
        return partyRepository.getParties()
    }

    fun chooseParty(party: Party) {
      partyRepository.pushParty(party)
          .subscribeOn(Schedulers.io())
          .observeOn(AndroidSchedulers.mainThread())
          .subscribe()
    }
}
