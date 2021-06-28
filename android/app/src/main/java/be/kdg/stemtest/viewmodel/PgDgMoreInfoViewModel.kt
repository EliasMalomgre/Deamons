package be.kdg.stemtest.viewmodel

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import be.kdg.stemtest.model.entity.Party
import be.kdg.stemtest.model.repositories.PartyRepository
import javax.inject.Inject

class PgDgMoreInfoViewModel @Inject constructor(private val partyRepository: PartyRepository) : ViewModel() {

    fun getParty(partyName: String): LiveData<Party> {
        return partyRepository.getParty(partyName)
    }

}
